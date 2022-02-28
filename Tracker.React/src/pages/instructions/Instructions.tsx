import { Table } from "antd";
import { observer } from "mobx-react";
import React, { FC, useContext } from "react";
import { StoreContext } from "../../App";
import { Instruction, InstructionRow } from "../../stores/InstructionsStore";

const Instructions: FC = observer(() => {
  const { instructionsStore, orgStructStore } = useContext(StoreContext);
  const dataWasLoaded =
    instructionsStore.instructions.size &&
    orgStructStore.orgStructElements.size;

  return (
    <>
      {dataWasLoaded ? (
        <Table<InstructionRow>
          key={Math.random()}
          dataSource={instructionsStore.instructionsRows}
          rowKey="id"
          defaultExpandAllRows
        >
          <Table.Column<Instruction> title="Поручение" dataIndex="name" />
          <Table.Column<Instruction>
            title="Создатель"
            dataIndex="creatorName"
          />
          <Table.Column<Instruction>
            title="Исполнитель"
            dataIndex="executorName"
          />
          <Table.Column<Instruction>
            title="Дэдлайн"
            dataIndex="deadline"
            width="10%"
          />
          <Table.Column<Instruction>
            title="Дата исполнения"
            dataIndex="execDate"
            width="10%"
          />
        </Table>
      ) : (
        "loading tree"
      )}
    </>
  );
});

export default Instructions;
