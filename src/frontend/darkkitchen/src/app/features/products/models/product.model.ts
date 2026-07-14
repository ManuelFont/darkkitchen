import { Category } from '../../categories/models/category.model';
import { Promotion } from '../../promotions/models/promotion.model';

export interface Product {
  id: string;
  imagesUrls: string[];
  name: string;
  description: string;
  activePromotion: Promotion | null;
  promotions: Promotion[];
  price: number;
  category: Category;
}
