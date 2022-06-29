import React, { FC, ReactNode, useState } from "react";
import { observer } from "mobx-react";
import { Table, Typography } from "antd";
import { UtilsStore, Util } from "./UtilsStore";
import { Status } from "./Status";

const { Text } = Typography;

const UtilsPage: FC = observer(() => {
  const [utilsStore] = useState(() => new UtilsStore());

  return (
    <Table<Util>
      key={Math.random()}
      dataSource={utilsStore.utils}
      rowKey="id"
    >
      <Table.Column<Util> title="Название" dataIndex="name" />
      <Table.Column<Util>
        width="10%"
        render={(_: string, record: Util): ReactNode =>
        (record.status === Status.Processing ?
          <Text disabled>Запустить</Text> :
          <a onClick={(): Promise<void> => utilsStore.run(record)}>Запустить</a>
        )}
      />
      <Table.Column<Util> title="Статус" dataIndex="status" />
    </Table>
  );
});

export default UtilsPage;
