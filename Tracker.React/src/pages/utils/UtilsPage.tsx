import React, { FC, ReactNode, useState } from "react";
import { observer } from "mobx-react";
import { Progress, Table, Typography } from "antd";
import { Util, ProgressableUtil } from "./Util";
import { UtilsStore } from "./UtilsStore";
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
        render={(_: string, util: Util): ReactNode =>
        (util.status === Status.Processing ?
          <Text disabled>Запустить</Text> :
          <a onClick={(): Promise<void> => util.run()}>Запустить</a>
        )}
      />
      <Table.Column<Util>
        title="Статус"
        dataIndex="status"
        width="10%"
        render={(_: string, util: Util): ReactNode =>
        (util instanceof ProgressableUtil && util.status === Status.Processing ?
          <Progress percent={util.progress} size="small" status="active" /> :
          <Text>{util.status}</Text>)
        }
      />
    </Table>
  );
});

export default UtilsPage;
