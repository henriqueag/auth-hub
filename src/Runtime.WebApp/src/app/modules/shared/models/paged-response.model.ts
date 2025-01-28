export interface PagedResponse<T> {
    items: T[];
    currentPage: number;
    pageSize: number;
    totalRecords: number;
    hasPrevious: boolean;
    hasNext: boolean;
}
