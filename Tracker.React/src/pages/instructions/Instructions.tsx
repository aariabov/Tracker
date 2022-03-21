import { Modal, Table } from "antd";
import { observer } from "mobx-react";
import React, { FC, ReactNode, useContext } from "react";
import { InstructionRow } from "./stores/InstructionsStore";
import { StoreContext } from "./InstructionsPage";

const Instructions: FC = observer(() => {
  const { instructionsStore, instructionStore, fullInstructionStore } =
    useContext(StoreContext);
  const dataWasLoaded = instructionsStore.instructions.length;

  const showConfirmModal = (instructionId: number): void => {
    const date = new Date();
    Modal.confirm({
      title: "Вы действительно хотите исполнить поручение?",
      content:
        "Будет установлена дата исполнения: " +
        date.toLocaleDateString("ru-RU"),
      async onOk() {
        await instructionStore.setExecutionDate(instructionId, date);
        instructionsStore.load();
      },
    });
  };

  const showInstructionCreationModal = (instructionId: number): void => {
    instructionStore.setParentId(instructionId);
    instructionStore.showModal();
  };

  return (
    <>
      {dataWasLoaded ? (
        <Table<InstructionRow>
          key={Math.random()}
          dataSource={instructionsStore.instructionsRows}
          rowKey="id"
          defaultExpandAllRows
        >
          <Table.Column<InstructionRow>
            title="Поручение"
            dataIndex="name"
            render={(text: string, record: InstructionRow): ReactNode => (
              <a
                onClick={(): Promise<void> =>
                  fullInstructionStore.show(record.id)
                }
              >
                {text}
              </a>
            )}
          />
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
          <Table.Column<InstructionRow>
            width="10%"
            render={(_: string, record: InstructionRow): ReactNode =>
              record.canBeExecuted && (
                <a onClick={(): void => showConfirmModal(record.id)}>
                  Исполнить
                </a>
              )
            }
          />
          <Table.Column<InstructionRow>
            width="10%"
            render={(_: string, record: InstructionRow): ReactNode =>
              record.canCreateChild && (
                <a
                  onClick={(): void => showInstructionCreationModal(record.id)}
                >
                  Создать дочернее
                </a>
              )
            }
          />
        </Table>
      ) : (
        "loading tree"
      )}
    </>
  );
});

export default Instructions;
