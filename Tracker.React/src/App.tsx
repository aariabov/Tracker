import React, { FC } from "react";
import { BrowserRouter, Outlet, Route, Routes } from "react-router-dom";
import InstructionsPage from "./pages/instructions/InstructionsPage";
import OrgStructPage from "./pages/orgStruct/OrgStructPage";
import EmployeeReportPage from "./pages/employeeReport/EmployeeReportPage";
import AppLayout from "./AppLayout";
import { ConfigProvider } from "antd";
import ruRU from "antd/lib/locale/ru_RU";
import Login from "./auth/Login";
import { observer } from "mobx-react";
import EmployeesReportPage from "./pages/employeesReport/EmployeesReportPage";
import RolesPage from "./pages/roles/RolesPage";
import { userStore } from "./auth/UserStore";
import Title from "antd/lib/typography/Title";

const AdminRoutes: FC = observer(() => {
  if (userStore.isAdmin) return <Outlet />;
  return <Title level={5}>Доступ запрещен!</Title>;
});

const AnalystRoutes: FC = observer(() => {
  if (userStore.isAnalyst) return <Outlet />;
  return <Title level={5}>Доступ запрещен!</Title>;
});

const App: FC = observer(() => {
  return (
    <ConfigProvider locale={ruRU}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<AppLayout />}>
            <Route path="instructions" element={<InstructionsPage />} />
            <Route path="/" element={<AdminRoutes />}>
              <Route path="org-struct" element={<OrgStructPage />} />
              <Route path="roles" element={<RolesPage />} />
            </Route>
            <Route path="/" element={<AnalystRoutes />}>
              <Route path="employee-report" element={<EmployeeReportPage />} />
              <Route
                path="employees-report"
                element={<EmployeesReportPage />}
              />
            </Route>
            <Route
              path="*"
              element={<Title level={5}>Страница не найдена!</Title>}
            />
          </Route>
        </Routes>
      </BrowserRouter>
    </ConfigProvider>
  );
});

export default App;
