import { Space } from "antd";
import React, { FC } from "react";
import Instructions from "./Instructions";
import InstructionForm from "./InstructionForm";

const InstructionsPage: FC = () => {
  return (
    <div className="app">
      <Space direction="vertical" size="large">
        <InstructionForm />
        <Instructions />
      </Space>
    </div>
  );
};

export default InstructionsPage;
