import { Space } from "antd";
import React, { FC, useContext, useEffect, useState } from "react";
import Instructions from "./Instructions";
import InstructionForm from "./InstructionForm";
import { StoreContext } from "../../App";
import { observer } from "mobx-react";
import FullInstruction from "./FullInstruction";
import { FullInstructionStore } from "./FullInstructionStore";

const InstructionsPage: FC = observer(() => {
  const { instructionsStore, orgStructStore } = useContext(StoreContext);
  const [fullInstructionStore] = useState(() => new FullInstructionStore());
  useEffect(() => {
    orgStructStore.load();
    instructionsStore.load();
  }, []);

  return (
    <div className="app">
      <Space direction="vertical" size="large" style={{ width: "100%" }}>
        <InstructionForm />
        <Instructions fullInstructionStore={fullInstructionStore} />
        <FullInstruction fullInstructionStore={fullInstructionStore} />
      </Space>
    </div>
  );
});

export default InstructionsPage;
