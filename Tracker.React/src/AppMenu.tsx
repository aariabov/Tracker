import React, { FC, ReactNode } from "react";
import { Menu } from "antd";
import { Link } from "react-router-dom";
import "antd/dist/antd.less";
import "./styles.less";
import { observer } from "mobx-react";
import { userStore } from "./auth/UserStore";
import SubMenu from "antd/lib/menu/SubMenu";

abstract class MenuItemBase {
  protected _key: string;
  protected _title: string;

  public constructor(key: string, title: string) {
    this._key = key;
    this._title = title;
  }

  protected abstract render(): ReactNode;
}

class MenuItem extends MenuItemBase {
  render(): ReactNode {
    return (
      <Menu.Item key={this._key}>
        <Link to={`/${this._key}`}>{this._title}</Link>
      </Menu.Item>
    );
  }
}

class SubMenuItem extends MenuItemBase {
  _children: MenuItem[];

  constructor(key: string, title: string, children: MenuItem[]) {
    super(key, title);
    this._children = children;
  }

  render(): ReactNode {
    return (
      <SubMenu key={this._key} title={this._title}>
        {this._children.map((child) => child.render())}
      </SubMenu>
    );
  }
}

const AppMenu: FC = observer(() => {
  const menu: MenuItem[] = [new MenuItem("instructions", "Поручения")];
  if (userStore.isAdmin) {
    const children = [
      new MenuItem("org-struct", "Оргструктура"),
      new MenuItem("roles", "Роли"),
    ];

    menu.push(new SubMenuItem("admin", "Админка", children));
  }

  if (userStore.isAnalyst) {
    const children = [
      new MenuItem("employee-report", "По сотруднику"),
      new MenuItem("employees-report", "По сотрудникам"),
    ];

    menu.push(new SubMenuItem("reports", "Отчеты", children));
  }

  return (
    <Menu theme="dark" mode="inline" defaultOpenKeys={["reports", "admin"]}>
      {menu.map((menuItem) => menuItem.render())}
    </Menu>
  );
});

export default AppMenu;
