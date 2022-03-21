import React, { FC, useState } from "react";
import { observer } from "mobx-react";
import OrgStruct from "./OrgStruct";
import OrgStructElementForm from "./OrgStructElementForm";
import { Space } from "antd";
import { OrgStructStore } from "../../stores/OrgStructStore";

const OrgStructPage: FC = observer(() => {
  const [orgStructStore] = useState(() => new OrgStructStore());

  return (
    <div className="app">
      <Space direction="vertical" size="large">
        <OrgStructElementForm orgStructStore={orgStructStore} />
        <OrgStruct orgStructStore={orgStructStore} />
      </Space>
    </div>
  );
});

export default OrgStructPage;
