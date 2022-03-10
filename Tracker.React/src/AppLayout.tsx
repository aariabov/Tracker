import React, { FC } from "react";
import { Outlet } from "react-router-dom";
import { Layout, Menu } from "antd";
import { Link } from "react-router-dom";
import "antd/dist/antd.less";
import "./styles.less";
import { observer } from "mobx-react";
import LoggedUser from "./components/LoggedUser/LoggedUser";

const { Content, Sider } = Layout;

const AppLayout: FC = observer(() => {
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
    </Layout>
  );
});

export default AppLayout;
