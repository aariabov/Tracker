import { Table } from "antd";
import React, { FC } from "react";
import { observer } from "mobx-react";
import { EmployeeReportStore } from "./EmployeeReportStore";
import { EmployeeReportRowVm } from "../../api/Api";

interface Props {
  store: EmployeeReportStore;
}

const EmployeeReportTable: FC<Props> = observer((props: Props) => {
  const { store } = props;
  const dataWasLoaded = store.rows.length > 0;

  return (
    <>
      {dataWasLoaded && (
        <Table<EmployeeReportRowVm>
          key={Math.random()}
          dataSource={store.rows}
          rowKey="id"
          defaultExpandAllRows
          style={{ width: 400 }}
        >
          <Table.Column<EmployeeReportRowVm>
            title="Статус"
            dataIndex="status"
          />
          <Table.Column<EmployeeReportRowVm>
            title="Кол-во"
            dataIndex="count"
            align="center"
          />
        </Table>
      )}
    </>
  );
});

export default EmployeeReportTable;
