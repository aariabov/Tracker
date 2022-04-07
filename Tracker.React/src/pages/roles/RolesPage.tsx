import React, { FC, useEffect, useState } from "react";
import { observer } from "mobx-react";
import RolesTable from "./RolesTable";
import { Space } from "antd";
import { RolesStore } from "./RolesStore";
import RoleForm from "./RoleForm";
import { RoleStore } from "./RoleStore";

const RolesPage: FC = observer(() => {
  const [rolesStore] = useState(() => new RolesStore());
  const [roleStore] = useState(() => new RoleStore());

  useEffect(() => {
    rolesStore.load();
  }, []);

  return (
    <Space direction="vertical" size="large">
      <RoleForm rolesStore={rolesStore} roleStore={roleStore} />
      <RolesTable rolesStore={rolesStore} roleStore={roleStore} />
    </Space>
  );
});

export default RolesPage;
