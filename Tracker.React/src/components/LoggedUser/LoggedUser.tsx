import React, { FC } from "react";
import { Avatar, Collapse, Menu, Space } from "antd";
import { RightOutlined } from "@ant-design/icons";
import { observer } from "mobx-react";
import { userStore } from "../../auth/UserStore";

const { Panel } = Collapse;

const LoggedUser: FC = observer(() => {
  return (
    <Collapse
      expandIconPosition="right"
      ghost
      expandIcon={({ isActive }): React.ReactElement => (
        <RightOutlined
          className="user-info__collapse-icon"
          rotate={isActive ? 90 : 0}
        />
      )}
    >
      <Panel
        header={
          <Space>
            <Avatar size="large" gap={2}>
              {userStore.email[0]}
            </Avatar>
            <span className="user-info__username">{userStore.email}</span>
          </Space>
        }
        key="1"
      >
        <Menu theme="dark" mode="inline" className="user-info__menu">
          <Menu.Item key="1">
            <div onClick={userStore.logout}>Выйти</div>
          </Menu.Item>
        </Menu>
        <div className="user-info__separator"></div>
      </Panel>
    </Collapse>
  );
});

export default LoggedUser;
