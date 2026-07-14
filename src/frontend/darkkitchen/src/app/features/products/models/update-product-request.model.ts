export interface UpdateProductRequest {
  name: string;
  description: string;
  imagesUrls: string[];
  price: number;
  categoryId: string;
}
