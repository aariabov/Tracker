import { Modal, Table } from "antd";
import React, { FC, ReactNode } from "react";
import { observer } from "mobx-react";
import { RolesStore } from "./RolesStore";
import { RoleStore } from "./RoleStore";
import { RoleVm } from "../../api/UsersApi";

interface Props {
  rolesStore: RolesStore;
  roleStore: RoleStore;
}

const RolesTable: FC<Props> = observer((props: Props) => {
  const { roleStore, rolesStore: store } = props;
  const dataWasLoaded = store.roles.length > 0;

  const showConfirmModal = (role: RoleVm): void => {
    Modal.confirm({
      title: "Вы действительно хотите удалить роль?",
      content: `Роль "${role.name}" будет удалена`,
      async onOk() {
        const result = await roleStore.deleteRole(role.id);
        if (result?.modelErrors) {
          Modal.error({
            title: "Произошла ошибка при удалении роли!",
            content: result.modelErrors.id,
          });
        } else {
          store.load();
        }
      },
    });
  };

  const editRole = (role: RoleVm): void => {
    roleStore.showModal(role);
  };

  return (
    <>
      {dataWasLoaded ? (
        <Table<RoleVm>
          key={Math.random()}
          dataSource={store.roles}
          rowKey="id"
          defaultExpandAllRows
          style={{ width: 400 }}
        >
          <Table.Column<RoleVm> title="Название" dataIndex="name" />
          <Table.Column<RoleVm>
            width="10%"
            render={(_: string, record: RoleVm): ReactNode => (
              <a onClick={(): void => editRole(record)}>Редактировать</a>
            )}
          />
          <Table.Column<RoleVm>
            width="10%"
            render={(_: string, record: RoleVm): ReactNode => (
              <a onClick={(): void => showConfirmModal(record)}>Удалить</a>
            )}
          />
        </Table>
      ) : (
        "loading tree"
      )}
    </>
  );
});

export default RolesTable;
