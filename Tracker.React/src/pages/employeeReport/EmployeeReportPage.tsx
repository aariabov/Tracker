import { Space } from "antd";
import React, { FC, useEffect, useState } from "react";
import { observer } from "mobx-react";
import { EmployeeReportStore } from "./EmployeeReportStore";
import EmployeeReportFilters from "./EmployeeReportFilters";
import EmployeeReportTable from "./EmployeeReportTable";
import { OrgStructStore } from "../../stores/OrgStructStore";

const EmployeeReportPage: FC = observer(() => {
  const [store] = useState(() => new EmployeeReportStore());
  const [orgStructStore] = useState(() => new OrgStructStore());

  useEffect(() => {
    orgStructStore.load();
  }, []);

  return (
    <Space direction="vertical" size="large">
      <EmployeeReportFilters store={store} orgStructStore={orgStructStore} />
      <EmployeeReportTable store={store} />
    </Space>
  );
});

export default EmployeeReportPage;
