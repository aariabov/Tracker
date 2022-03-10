import { Space } from "antd";
import React, { FC, useContext, useEffect } from "react";
import Instructions from "./Instructions";
import InstructionForm from "./InstructionForm";
import { StoreContext } from "../../App";
import { observer } from "mobx-react";

const InstructionsPage: FC = observer(() => {
  const { instructionsStore, orgStructStore } = useContext(StoreContext);
  useEffect(() => {
    orgStructStore.load();
    instructionsStore.load();
  }, []);

  return (
    <div className="app">
      <Space direction="vertical" size="large">
        <InstructionForm />
        <Instructions />
      </Space>
    </div>
  );
});

export default InstructionsPage;
