import { Button, DatePicker, Form } from "antd";
import React, { FC } from "react";
import { observer } from "mobx-react";
import { EmployeesReportStore } from "./EmployeesReportStore";

const { RangePicker } = DatePicker;

interface Props {
  store: EmployeesReportStore;
}

const EmployeesReportFilters: FC<Props> = observer((props: Props) => {
  const { store } = props;

  return (
    <Form layout={"inline"} autoComplete="off">
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

export default EmployeesReportFilters;
