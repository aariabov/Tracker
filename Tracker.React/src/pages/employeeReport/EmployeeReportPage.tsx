import { Space } from "antd";
import React, { FC, useState } from "react";
import { observer } from "mobx-react";
import { EmployeeReportStore } from "./EmployeeReportStore";
import EmployeeReportFilters from "./EmployeeReportFilters";
import EmployeeReportTable from "./EmployeeReportTable";

const EmployeeReportPage: FC = observer(() => {
  const [store] = useState(() => new EmployeeReportStore());

  return (
    <div className="app">
      <Space direction="vertical" size="large">
        <EmployeeReportFilters store={store} />
        <EmployeeReportTable store={store} />
      </Space>
    </div>
  );
});

export default EmployeeReportPage;
