import React, { FC, useContext, useEffect } from "react";
import { observer } from "mobx-react";
import OrgStruct from "./OrgStruct";
import OrgStructElementForm from "./OrgStructElementForm";
import { Space } from "antd";
import { StoreContext } from "../../App";

const OrgStructPage: FC = observer(() => {
  const { orgStructStore } = useContext(StoreContext);
  useEffect(() => {
    orgStructStore.load();
  }, []);

  return (
    <div className="app">
      <Space direction="vertical" size="large">
        <OrgStructElementForm />
        <OrgStruct />
      </Space>
    </div>
  );
});

export default OrgStructPage;
