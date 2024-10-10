import { ManufacturerReviewsModel } from "./manufacturer.reviews.model";

export interface ProfileManufacturerModel {
    name : string;
    reviews: ManufacturerReviewsModel[];
}
