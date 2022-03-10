import React, { FC, createContext } from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import InstructionsPage from "./pages/instructions/InstructionsPage";
import OrgStructPage from "./pages/orgStruct/OrgStructPage";
import AppLayout from "./AppLayout";
import { ConfigProvider } from "antd";
import ruRU from "antd/lib/locale/ru_RU";
import { mainStore } from "./stores/MainStore";
import Login from "./auth/Login";
import { userStore } from "./auth/UserStore";
import { observer } from "mobx-react";

export const StoreContext = createContext(mainStore);

const App: FC = observer(() => {
  if (!userStore.token) {
    return <Login />;
  }

  return (
    <ConfigProvider locale={ruRU}>
      <StoreContext.Provider value={mainStore}>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<AppLayout />}>
              <Route path="instructions" element={<InstructionsPage />} />
              <Route path="org-struct" element={<OrgStructPage />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </StoreContext.Provider>
    </ConfigProvider>
  );
});

export default App;
