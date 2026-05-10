import api from './client';
import type { Auction, AuctionDetail, PagedResult, Bid } from '../types';

export const getAuctions = async (
  page = 1,
  pageSize = 10,
  category?: number,
  status?: number,
  sortBy?: string
): Promise<PagedResult<Auction>> => {
  const params = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  if (category !== undefined) params.append('category', String(category));
  if (status !== undefined) params.append('status', String(status));
  if (sortBy) params.append('sortBy', sortBy);

  const { data } = await api.get<PagedResult<Auction>>(`/auctions?${params}`);
  return data;
};

export const getAuction = async (id: string): Promise<AuctionDetail> => {
  const { data } = await api.get<AuctionDetail>(`/auctions/${id}`);
  return data;
};

export const createAuction = async (auction: {
  title: string;
  description: string;
  category: number;
  startingPrice: number;
  startDate: string;
  endDate: string;
}): Promise<Auction> => {
  const { data } = await api.post<Auction>('/auctions', auction);
  return data;
};

export const placeBid = async (auctionId: string, amount: number): Promise<Bid> => {
  const { data } = await api.post<Bid>(`/auctions/${auctionId}/bids`, { amount });
  return data;
};
