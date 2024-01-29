CREATE EXTENSION postgres_fdw;

CREATE SERVER foreign_server
        FOREIGN DATA WRAPPER postgres_fdw
        OPTIONS (host 'localhost', port '5432', dbname 'tracker_instructions');
        
CREATE USER MAPPING FOR postgres
        SERVER foreign_server
        OPTIONS (user 'postgres', password '123123asd@');

CREATE SCHEMA tracker;

IMPORT FOREIGN SCHEMA public
LIMIT TO (users, instructions)
FROM SERVER foreign_server INTO tracker;

insert into users (id, user_name)
select id, user_name from tracker.users;

insert into instructions (id, "name", creator_id, executor_id, deadline, exec_date)
select id, "name", parent_id, tree_path, creator_id, executor_id, deadline, exec_date from tracker.instructions;
SELECT setval('instructions_id_seq', (select max(id) from instructions));

insert into instructions_closures (parent_id, id, depth)
select parent_id, id, depth from tracker.instructions_closures;