export interface CartItemView {
  productId: string;
  name: string;
  imageUrl: string;
  quantity: number;
  unitPriceLabel: string;
  lineLabel: string;
  originalLineLabel: string | null;
}
