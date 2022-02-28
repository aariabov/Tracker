import React, { FC, useContext } from "react";
import { observer } from "mobx-react";
import { Tree } from "antd";
import { StoreContext } from "../../App";

const OrgStruct: FC = observer(() => {
  const { orgStructStore } = useContext(StoreContext);

  return (
    <div>
      <h3>Сотрудники ({orgStructStore.orgStructElements.size})</h3>
      <Tree
        key={Math.random()}
        showLine={{ showLeafIcon: false }}
        defaultExpandAll
        showIcon={false}
        treeData={orgStructStore.orgStructTreeData}
      />
    </div>
  );
});

export default OrgStruct;
