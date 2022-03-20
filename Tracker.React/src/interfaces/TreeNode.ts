export default interface TreeNode<T extends number | string> {
  key: T;
  value: T;
  title: string;
  parentId?: T;
  children: TreeNode<T>[];
}
