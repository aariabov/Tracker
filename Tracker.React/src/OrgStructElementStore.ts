import { action, makeObservable, observable } from 'mobx';
import { OrgStructElement } from './OrgStructStore';

export default class OrgStructElementStore {

    id: number = 0;
    name: string = '';
    parentId: number | undefined = undefined;

    constructor() {
        makeObservable(this, {
            id: observable,
            name: observable,
            parentId: observable,
            setName: action,
            setParentId: action,
        });
    }

    setName = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.name = e.target.value;
    };

    setParentId = (value: number) => {
        this.parentId = value;
    };

    save = async (): Promise<OrgStructElement> => {
        const body = JSON.stringify({
            name: this.name,
            parentId: this.parentId
        });

        const rawResponse = await fetch('api/OrgStruct', {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
            body: body
        });

        const content = await rawResponse.json();
        return content;
    };
}