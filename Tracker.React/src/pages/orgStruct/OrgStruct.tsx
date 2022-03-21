import React, { FC } from "react";
import { observer } from "mobx-react";
import { Tree } from "antd";
import { OrgStructStore } from "../../stores/OrgStructStore";

interface Props {
  orgStructStore: OrgStructStore;
}

const OrgStruct: FC<Props> = observer((props: Props) => {
  const { orgStructStore } = props;

  return (
    <div>
      <h3>Сотрудники ({orgStructStore.orgStructElements.length})</h3>
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
