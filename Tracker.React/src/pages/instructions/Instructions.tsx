import { Modal, Table, TablePaginationConfig } from "antd";
import { observer } from "mobx-react";
import React, { FC, ReactNode, useContext } from "react";
import { InstructionRow } from "./stores/InstructionsStore";
import { StoreContext } from "./InstructionsPage";
import { SortOrder } from "antd/lib/table/interface";

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

  const pagingConfig: TablePaginationConfig = {
    pageSize: instructionsStore.perPage,
    total: instructionsStore.totalInstructions,
    current: instructionsStore.page,
    showSizeChanger: true,
    pageSizeOptions: ["5", "10", "50", "100"]
  }

  const sortOrder = (colKey: string): SortOrder | undefined =>
    instructionsStore.sortedInfo.columnKey === colKey ? instructionsStore.sortedInfo.order : null;

  return (
    <>
      {dataWasLoaded ? (
        <Table<InstructionRow>
          key={Math.random()}
          dataSource={instructionsStore.instructionsRows}
          rowKey="id"
          defaultExpandAllRows
          pagination={pagingConfig}
          showSorterTooltip={false}
          onChange={instructionsStore.onChange}
        >
          <Table.Column<InstructionRow>
            key="name"
            dataIndex="name"
            title="Поручение"
            sorter={true}
            sortOrder={sortOrder("name")}
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
            key="creatorName"
            dataIndex="creatorName"
            title="Создатель"
            width="10%"
            sorter={true}
            sortOrder={sortOrder('creatorName')}
          />
          <Table.Column<InstructionRow>
            key="executorName"
            dataIndex="executorName"
            title="Исполнитель"
            width="10%"
            sorter={true}
            sortOrder={sortOrder('executorName')}
          />
          <Table.Column<InstructionRow>
            key="deadline"
            dataIndex="deadline"
            title="Дэдлайн"
            width="10%"
            sorter={true}
            sortOrder={sortOrder('deadline')}
            render={(deadline: string): ReactNode =>
              new Date(deadline).toLocaleDateString("ru-RU")
            }
          />
          <Table.Column<InstructionRow>
            key="execDate"
            dataIndex="execDate"
            title="Дата исполнения"
            width="10%"
            sorter={true}
            sortOrder={sortOrder('execDate')}
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
