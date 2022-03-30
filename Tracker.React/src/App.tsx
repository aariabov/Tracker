import React, { FC } from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import InstructionsPage from "./pages/instructions/InstructionsPage";
import OrgStructPage from "./pages/orgStruct/OrgStructPage";
import EmployeeReportPage from "./pages/employeeReport/EmployeeReportPage";
import AppLayout from "./AppLayout";
import { ConfigProvider } from "antd";
import ruRU from "antd/lib/locale/ru_RU";
import Login from "./auth/Login";
import { observer } from "mobx-react";
import EmployeesReportPage from "./pages/employeesReport/EmployeesReportPage";

const App: FC = observer(() => {
  return (
    <ConfigProvider locale={ruRU}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<AppLayout />}>
            <Route path="instructions" element={<InstructionsPage />} />
            <Route path="org-struct" element={<OrgStructPage />} />
            <Route path="employee-report" element={<EmployeeReportPage />} />
            <Route path="employees-report" element={<EmployeesReportPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </ConfigProvider>
  );
});

export default App;
