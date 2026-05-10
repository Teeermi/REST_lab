import { Link } from 'react-router-dom';
import type { Auction } from '../types';
import { CategoryLabels, StatusLabels } from '../types';

interface Props {
  auction: Auction;
}

export default function AuctionCard({ auction }: Props) {
  const isActive = auction.status === 1;
  const timeLeft = new Date(auction.endDate).getTime() - Date.now();
  const daysLeft = Math.max(0, Math.floor(timeLeft / (1000 * 60 * 60 * 24)));

  return (
    <Link
      to={`/auction/${auction.id}`}
      className="block bg-white border border-gray-200 rounded-xl p-5 hover:shadow-lg transition-shadow"
    >
      <div className="flex justify-between items-start mb-3">
        <span className="text-xs px-2 py-1 bg-gray-100 text-gray-600 rounded">
          {CategoryLabels[auction.category]}
        </span>
        <span
          className={`text-xs px-2 py-1 rounded ${
            isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-600'
          }`}
        >
          {StatusLabels[auction.status]}
        </span>
      </div>

      <h3 className="text-lg font-medium text-gray-900 mb-2 line-clamp-2">
        {auction.title}
      </h3>

      <p className="text-sm text-gray-500 mb-4 line-clamp-2">
        {auction.description}
      </p>

      <div className="flex justify-between items-end">
        <div>
          <p className="text-xs text-gray-500">Aktualna cena</p>
          <p className="text-xl font-semibold text-blue-600">
            {auction.currentPrice.toFixed(2)} zł
          </p>
        </div>
        {isActive && (
          <p className="text-sm text-gray-500">
            {daysLeft > 0 ? `${daysLeft} dni` : 'Kończy się dziś'}
          </p>
        )}
      </div>
    </Link>
  );
}
