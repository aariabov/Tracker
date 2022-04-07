import { Space } from "antd";
import React, { FC, useState } from "react";
import { observer } from "mobx-react";
import { EmployeesReportStore } from "./EmployeesReportStore";
import EmployeesReportFilters from "./EmployeesReportFilters";
import EmployeesReportTable from "./EmployeesReportTable";

const EmployeesReportPage: FC = observer(() => {
  const [store] = useState(() => new EmployeesReportStore());

  return (
    <Space direction="vertical" size="large">
      <EmployeesReportFilters store={store} />
      <EmployeesReportTable store={store} />
    </Space>
  );
});

export default EmployeesReportPage;
