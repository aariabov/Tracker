/**
 * Создает дерево из плоского id, parentId списка
 * @param items плоский список id, parentId
 * @param mapFunc маппинг src на dst, решает проблему нейминга (например, не id, а key)
 * @returns дерево
 */
export function listToTree<
  TKey,
  TListElement extends ListElement<TKey>,
  TTreeElement extends TreeElement<TKey>
>(
  items: Iterable<TListElement>,
  mapFunc: (value: TListElement) => TTreeElement
): TTreeElement[] {
  const map = new Map<TKey, TTreeElement>();
  const result: TTreeElement[] = [];

  for (const item of items) {
    map.set(item.id, mapFunc(item));
  }

  for (const item of items) {
    const currentNode = map.get(item.id)!;

    if (item.parentId && map.get(item.parentId)) {
      const parentNode = map.get(item.parentId)!;
      parentNode.children.push(currentNode);
    } else {
      result.push(currentNode);
    }
  }

  return result;
}

interface ListElement<KeyT> {
  id: KeyT;
  parentId?: KeyT;
}

interface TreeElement<KeyT> {
  children: TreeElement<KeyT>[];
}
