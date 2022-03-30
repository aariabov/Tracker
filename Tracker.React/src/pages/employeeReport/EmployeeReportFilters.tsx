import { Button, DatePicker, Form, TreeSelect } from "antd";
import React, { FC, useState } from "react";
import { observer } from "mobx-react";
import { EmployeeReportStore } from "./EmployeeReportStore";
import { OrgStructStore } from "../../stores/OrgStructStore";

const { RangePicker } = DatePicker;

interface Props {
  store: EmployeeReportStore;
}

const EmployeeReportFilters: FC<Props> = observer((props: Props) => {
  const [orgStructStore] = useState(() => new OrgStructStore());
  const { store } = props;

  return (
    <Form layout={"inline"} autoComplete="off">
      <Form.Item
        label="Исполнитель"
        validateStatus={store.executorIdError && "error"}
        help={store.executorIdError}
      >
        <TreeSelect
          dropdownStyle={{
            maxHeight: 400,
            minWidth: 300,
            overflow: "auto",
          }}
          treeData={orgStructStore.orgStructTreeData}
          treeDefaultExpandAll
          onChange={store.setExecutorId}
          style={{ width: 288 }}
          allowClear
        />
      </Form.Item>
      <Form.Item
        label="Дедлайн"
        validateStatus={store.periodError && "error"}
        help={store.periodError}
      >
        <RangePicker format={"DD.MM.YYYY"} onChange={store.setPeriod} />
      </Form.Item>
      <Form.Item>
        <Button type="primary" onClick={store.load}>
          Показать
        </Button>
      </Form.Item>
    </Form>
  );
});

export default EmployeeReportFilters;
