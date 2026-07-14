export interface CreateProductRequest {
  name: string;
  description: string;
  imagesUrls: string[];
  price: number;
  categoryId: string;
}
