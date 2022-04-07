import React, { FC } from "react";
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Layout, Modal } from "antd";
import "antd/dist/antd.less";
import "./styles.less";
import { observer } from "mobx-react";
import LoggedUser from "./components/LoggedUser/LoggedUser";
import { errorStore } from "./stores/ErrorStore";
import { userStore } from "./auth/UserStore";
import AppMenu from "./AppMenu";

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
        <AppMenu />
      </Sider>
      <Layout>
        <Content style={{ padding: "16px" }}>
          <Outlet />
        </Content>
      </Layout>
      {errorStore.error && showErrorModal()}
    </Layout>
  );
});

export default AppLayout;
