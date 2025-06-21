export function entityIsNotFound<T extends { id: number }>(entity: T) : boolean {
    if(entity === null || entity === undefined || (typeof entity === 'object' && Object.keys(entity).length === 0)) {
        return true;
    }

    return false;
}