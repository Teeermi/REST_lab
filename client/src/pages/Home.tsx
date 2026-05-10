import { useState, useEffect } from 'react';
import { getAuctions } from '../api/auctions';
import AuctionCard from '../components/AuctionCard';
import type { Auction } from '../types';
import { CategoryLabels } from '../types';

export default function Home() {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [category, setCategory] = useState<number | undefined>();
  const [sortBy, setSortBy] = useState('date_desc');

  useEffect(() => {
    const fetchAuctions = async () => {
      setLoading(true);
      try {
        const data = await getAuctions(page, 12, category, 1, sortBy);
        setAuctions(data.items);
        setTotalPages(Math.ceil(data.totalCount / 12));
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchAuctions();
  }, [page, category, sortBy]);

  return (
    <div className="max-w-6xl mx-auto px-6 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-2xl font-semibold text-gray-900">Aktywne aukcje</h1>

        <div className="flex gap-4">
          <select
            value={category ?? ''}
            onChange={(e) => setCategory(e.target.value ? Number(e.target.value) : undefined)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm"
          >
            <option value="">Wszystkie kategorie</option>
            {Object.entries(CategoryLabels).map(([key, label]) => (
              <option key={key} value={key}>{label}</option>
            ))}
          </select>

          <select
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm"
          >
            <option value="date_desc">Najnowsze</option>
            <option value="date">Najstarsze</option>
            <option value="price">Cena rosnąco</option>
            <option value="price_desc">Cena malejąco</option>
          </select>
        </div>
      </div>

      {loading ? (
        <div className="text-center py-12 text-gray-500">Ładowanie...</div>
      ) : auctions.length === 0 ? (
        <div className="text-center py-12 text-gray-500">Brak aukcji</div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {auctions.map((auction) => (
              <AuctionCard key={auction.id} auction={auction} />
            ))}
          </div>

          {totalPages > 1 && (
            <div className="flex justify-center gap-2 mt-8">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className="px-4 py-2 border border-gray-300 rounded-lg disabled:opacity-50"
              >
                Poprzednia
              </button>
              <span className="px-4 py-2 text-gray-600">
                {page} / {totalPages}
              </span>
              <button
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="px-4 py-2 border border-gray-300 rounded-lg disabled:opacity-50"
              >
                Następna
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
}
