import { Table } from "antd";
import React, { FC } from "react";
import { observer } from "mobx-react";
import { EmployeesReportStore, ReportRow } from "./EmployeesReportStore";

interface Props {
  store: EmployeesReportStore;
}

const EmployeesReportTable: FC<Props> = observer((props: Props) => {
  const { store } = props;
  const dataWasLoaded = store.rows.length > 0;

  return (
    <>
      {dataWasLoaded && (
        <Table<ReportRow>
          key={Math.random()}
          dataSource={store.rows}
          rowKey="id"
          defaultExpandAllRows
        >
          <Table.Column<ReportRow>
            title="Исполнитель / Статус"
            dataIndex="executor"
          />
          <Table.Column<ReportRow>
            title="В работе"
            dataIndex="inWorkCount"
            align="center"
          />
          <Table.Column<ReportRow>
            title="В работе просрочено"
            dataIndex="inWorkOverdueCount"
            align="center"
          />
          <Table.Column<ReportRow>
            title="Выполнено в срок"
            dataIndex="completedCount"
            align="center"
          />
          <Table.Column<ReportRow>
            title="Выполнено с нарушением срока"
            dataIndex="completedOverdueCount"
            align="center"
          />
        </Table>
      )}
    </>
  );
});

export default EmployeesReportTable;
