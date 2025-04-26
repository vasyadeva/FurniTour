export interface ItemFilterModel {
  colorID: number;
  categoryID: number;
  manufacturerID: number;
  masterID: string;
  minPrice: number;
  maxPrice: number;
  searchString?: string;
}