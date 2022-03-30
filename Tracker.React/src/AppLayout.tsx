import React, { FC } from "react";
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Layout, Menu, Modal } from "antd";
import { Link } from "react-router-dom";
import "antd/dist/antd.less";
import "./styles.less";
import { observer } from "mobx-react";
import LoggedUser from "./components/LoggedUser/LoggedUser";
import { errorStore } from "./stores/ErrorStore";
import { userStore } from "./auth/UserStore";
import SubMenu from "antd/lib/menu/SubMenu";

const { Content, Sider } = Layout;

const AppLayout: FC = observer(() => {
  const showErrorModal = (): void => {
    Modal.error({
      title: "Произошла ошибка",
      content: errorStore.error,
      okText: "Закрыть",
      onOk: (): void => errorStore.clearError(),
    });
  };

  if (!userStore.token) {
    let location = useLocation();
    return <Navigate to="/login" state={{ from: location }} />;
  }

  return (
    <Layout>
      <Sider style={{ height: "100vh" }}>
        <LoggedUser />
        <Menu theme="dark" mode="inline" defaultOpenKeys={["reports"]}>
          <Menu.Item key="1">
            <Link to="/org-struct">Оргструктура</Link>
          </Menu.Item>
          <Menu.Item key="2">
            <Link to="/instructions">Поручения</Link>
          </Menu.Item>
          <SubMenu key="reports" title="Отчеты">
            <Menu.Item key="3">
              <Link to="/employee-report">По сотруднику</Link>
            </Menu.Item>
            <Menu.Item key="4">
              <Link to="/employees-report">По сотрудникам</Link>
            </Menu.Item>
          </SubMenu>
        </Menu>
      </Sider>
      <Layout>
        <Content>
          <Outlet />
        </Content>
      </Layout>
      {errorStore.error && showErrorModal()}
    </Layout>
  );
});

export default AppLayout;
