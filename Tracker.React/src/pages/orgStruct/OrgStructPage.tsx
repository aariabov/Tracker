import React, { FC, useEffect, useState } from "react";
import { observer } from "mobx-react";
import OrgStructTable from "./OrgStructTable";
import OrgStructElementForm from "./OrgStructElementForm";
import { Space } from "antd";
import { OrgStructStore } from "../../stores/OrgStructStore";
import { OrgStructElementStore } from "./OrgStructElementStore";
import { RolesStore } from "../roles/RolesStore";

const OrgStructPage: FC = observer(() => {
  const [orgStructStore] = useState(() => new OrgStructStore());
  const [orgStructElementStore] = useState(() => new OrgStructElementStore());
  const [rolesStore] = useState(() => new RolesStore());

  useEffect(() => {
    rolesStore.load();
    orgStructStore.load();
  }, []);

  return (
    <Space direction="vertical" size="large" style={{ width: "70%" }}>
      <OrgStructElementForm
        rolesStore={rolesStore}
        orgStructStore={orgStructStore}
        orgStructElementStore={orgStructElementStore}
      />
      <OrgStructTable
        orgStructStore={orgStructStore}
        orgStructElementStore={orgStructElementStore}
      />
    </Space>
  );
});

export default OrgStructPage;
