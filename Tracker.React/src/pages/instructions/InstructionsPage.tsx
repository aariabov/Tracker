import { Space } from "antd";
import React, { createContext, FC, useContext, useEffect } from "react";
import Instructions from "./Instructions";
import InstructionForm from "./InstructionForm";
import { observer } from "mobx-react";
import FullInstruction from "./FullInstruction";
import { userStore } from "../../auth/UserStore";
import { pageStore } from "./stores/PageStore";

export const StoreContext = createContext(pageStore);

const InstructionsPage: FC = observer(() => {
  const { instructionsStore, orgStructStore } = useContext(StoreContext);
  useEffect(() => {
    orgStructStore.load();
    instructionsStore.getTotalInstructions();
    instructionsStore.load();
  }, []);

  return (
    <StoreContext.Provider value={pageStore}>
      <Space direction="vertical" size="large" style={{ width: "100%" }}>
        {userStore.isUserBoss && <InstructionForm />}
        <Instructions />
        <FullInstruction />
      </Space>
    </StoreContext.Provider>
  );
});

export default InstructionsPage;
