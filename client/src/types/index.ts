export interface User {
  id: string;
  email: string;
  username: string;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface Auction {
  id: string;
  title: string;
  description: string;
  category: number;
  startingPrice: number;
  currentPrice: number;
  startDate: string;
  endDate: string;
  status: number;
  ownerId: string;
  ownerUsername: string;
}

export interface AuctionDetail extends Auction {
  bids: Bid[];
}

export interface Bid {
  id: string;
  amount: number;
  createdAt: string;
  bidderId: string;
  bidderUsername: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export const CategoryLabels: Record<number, string> = {
  0: 'Elektronika',
  1: 'Moda',
  2: 'Dom',
  3: 'Sport',
  4: 'Inne',
};

export const StatusLabels: Record<number, string> = {
  0: 'Szkic',
  1: 'Aktywna',
  2: 'Zakończona',
  3: 'Anulowana',
};
