import React, { FC } from "react";
import { Outlet } from "react-router-dom";
import { Button, Layout, Menu, Modal } from "antd";
import { Link } from "react-router-dom";
import "antd/dist/antd.less";
import "./styles.less";
import { observer } from "mobx-react";
import LoggedUser from "./components/LoggedUser/LoggedUser";
import { errorStore } from "./stores/ErrorStore";

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

  return (
    <Layout>
      <Sider style={{ height: "100vh" }}>
        <LoggedUser />
        <Menu theme="dark" mode="inline">
          <Menu.Item key="1">
            <Link to="/org-struct">Оргструктура</Link>
          </Menu.Item>
          <Menu.Item key="2">
            <Link to="/instructions">Поручения</Link>
          </Menu.Item>
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
