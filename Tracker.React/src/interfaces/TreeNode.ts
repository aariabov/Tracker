export default interface TreeNode {
  key: number;
  value: number;
  title: string;
  parentId?: number;
  children: TreeNode[];
}
