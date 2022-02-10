import { makeObservable, observable, computed, action } from 'mobx';

export class OrgStructStore {

    orgStructElements: OrgStructElement[] = [];

    constructor() {
        makeObservable(this, {
            orgStructElements: observable,
            orgStructTreeData: computed,
            load: action,
        });
    }

    get orgStructTreeData(): TreeNode[] {
        return this.listToTree(this.orgStructElements);
    }

    listToTree = (items: OrgStructElement[]): TreeNode[] => {
        const map = new Map(items.map((item, idx) => [item.id, idx]));
        const list: TreeNode[] = items.map(e => ({
            key: e.id,
            value: e.id,
            title: e.name,
            parentId: e.parentId,
            children: []
        }));
        
        const roots: TreeNode[] = [];
        for (let i = 0; i < list.length; i += 1) {
            const node = list[i];
            if (node.parentId) {
                const parentIdx = map.get(node.parentId)!;
                list[parentIdx].children!.push(node);
            } else {
                roots.push(node);
            }
        }

        return roots;
    };

    load = async () => {
        const url = 'api/OrgStruct';
        const response = await fetch(url);
        const data = await response.json();
        
        this.orgStructElements = data;
    };
}

interface TreeNode {
    key: number,
    value: number,
    title: string,
    parentId?: number,
    children: TreeNode[]
}

export interface OrgStructElement {
    id: number;
    name: string;
    parentId?: number;
}
