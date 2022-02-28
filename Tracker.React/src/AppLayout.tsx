import ReactDOM from "react-dom";
import React, { FC } from "react";
import { BrowserRouter, Route, Routes, Outlet } from "react-router-dom";
import App from "./App";
import InstructionsPage from "./pages/instructions/InstructionsPage";
import OrgStructPage from "./pages/orgStruct/OrgStructPage";
import { Layout, Menu } from "antd";
import { Link } from "react-router-dom";
import "antd/dist/antd.less";
import "./styles.less";

const { Header, Content, Footer, Sider } = Layout;

const AppLayout: FC = () => {
  return (
    <Layout>
      <Sider style={{ height: "100vh" }}>
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
};

export default AppLayout;
