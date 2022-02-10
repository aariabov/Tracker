import { OrgStructStore } from './OrgStructStore';
import OrgStructElementStore from './OrgStructElementStore';

export default class MainStore {
    orgStructStore: OrgStructStore;
    orgStructElementStore: OrgStructElementStore;

    constructor() {
        this.orgStructElementStore = new OrgStructElementStore();
        this.orgStructStore = new OrgStructStore();
        this.orgStructStore.load();
    }

    save = async () => {
        const createdElement = await this.orgStructElementStore.save();
        if (createdElement.id > 0) {
            this.orgStructStore.load();
            this.orgStructElementStore = new OrgStructElementStore();
        }
    };
}