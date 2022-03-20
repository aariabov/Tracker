import { Button, Drawer, Table } from "antd";
import { observer } from "mobx-react";
import React, { FC, ReactNode } from "react";
import { FullInstructionStore, InstructionRow } from "./FullInstructionStore";

interface Props {
  fullInstructionStore: FullInstructionStore;
}

const FullInstruction: FC<Props> = observer((props: Props) => {
  const { fullInstructionStore: store } = props;
  const dataWasLoaded = store.instructionsRows.length;

  return (
    <>
      <Drawer
        title="Полное поручение"
        placement="right"
        width="70%"
        onClose={store.close}
        visible={store.isDrawerVisible}
        extra={<Button onClick={store.close}>Закрыть</Button>}
      >
        {dataWasLoaded ? (
          <Table<InstructionRow>
            key={Math.random()}
            dataSource={store.instructionsRows}
            rowKey="id"
            defaultExpandAllRows
          >
            <Table.Column<InstructionRow> title="Поручение" dataIndex="name" />
            <Table.Column<InstructionRow>
              title="Создатель"
              dataIndex="creatorName"
              width="10%"
            />
            <Table.Column<InstructionRow>
              title="Исполнитель"
              dataIndex="executorName"
              width="10%"
            />
            <Table.Column<InstructionRow>
              title="Дэдлайн"
              dataIndex="deadline"
              width="10%"
              render={(deadline: string): ReactNode =>
                new Date(deadline).toLocaleDateString("ru-RU")
              }
            />
            <Table.Column<InstructionRow>
              title="Дата исполнения"
              dataIndex="execDate"
              width="10%"
              render={(execDate: string): ReactNode =>
                execDate ? new Date(execDate).toLocaleDateString("ru-RU") : ""
              }
            />
            <Table.Column<InstructionRow>
              title="Статус"
              dataIndex="status"
              width="10%"
            />
          </Table>
        ) : (
          "loading tree"
        )}
      </Drawer>
    </>
  );
});

export default FullInstruction;
