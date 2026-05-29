import { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { getAuction, placeBid } from '../api/auctions';
import { getApiErrorMessage } from '../api/errors';
import { useAuth } from '../hooks/useAuth';
import type { AuctionDetail as AuctionDetailType } from '../types';
import { CategoryLabels, StatusLabels } from '../types';

export default function AuctionDetail() {
  const { id } = useParams<{ id: string }>();
  const { isAuthenticated, user } = useAuth();
  const [auction, setAuction] = useState<AuctionDetailType | null>(null);
  const [loading, setLoading] = useState(true);
  const [bidAmount, setBidAmount] = useState('');
  const [bidError, setBidError] = useState('');
  const [bidding, setBidding] = useState(false);

  useEffect(() => {
    const fetchAuction = async () => {
      if (!id) return;
      try {
        const data = await getAuction(id);
        setAuction(data);
        setBidAmount((data.currentPrice + 1).toFixed(2));
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchAuction();
  }, [id]);

  const handleBid = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id || !auction) return;

    setBidError('');
    setBidding(true);

    try {
      await placeBid(id, parseFloat(bidAmount));
      const updated = await getAuction(id);
      setAuction(updated);
      setBidAmount((updated.currentPrice + 1).toFixed(2));
    } catch (err: unknown) {
      setBidError(getApiErrorMessage(err, 'Błąd składania oferty'));
    } finally {
      setBidding(false);
    }
  };

  if (loading) {
    return <div style={{ textAlign: 'center', padding: '48px', color: '#6b7280' }}>Ładowanie...</div>;
  }

  if (!auction) {
    return <div style={{ textAlign: 'center', padding: '48px', color: '#6b7280' }}>Aukcja nie znaleziona</div>;
  }

  const isActive = auction.status === 1;
  const isOwner = user?.id === auction.ownerId;
  const canBid = isAuthenticated && isActive && !isOwner;

  return (
    <div style={{ maxWidth: '896px', margin: '0 auto', padding: '32px 24px' }}>
      <div style={{ background: '#fff', border: '1px solid #e5e7eb', borderRadius: '12px', padding: '32px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '24px' }}>
          <span style={{ fontSize: '14px', padding: '4px 12px', background: '#f3f4f6', color: '#4b5563', borderRadius: '4px' }}>
            {CategoryLabels[auction.category]}
          </span>
          <span style={{
            fontSize: '14px',
            padding: '4px 12px',
            background: isActive ? '#dcfce7' : '#f3f4f6',
            color: isActive ? '#15803d' : '#4b5563',
            borderRadius: '4px'
          }}>
            {StatusLabels[auction.status]}
          </span>
        </div>

        <h1 style={{ fontSize: '30px', fontWeight: 600, color: '#111827', marginBottom: '16px' }}>
          {auction.title}
        </h1>

        <p style={{ color: '#4b5563', marginBottom: '24px' }}>{auction.description}</p>

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginBottom: '32px' }}>
          <div>
            <p style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>Cena wywoławcza</p>
            <p style={{ fontSize: '18px', color: '#374151' }}>{auction.startingPrice.toFixed(2)} zł</p>
          </div>
          <div>
            <p style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>Aktualna cena</p>
            <p style={{ fontSize: '24px', fontWeight: 600, color: '#2563eb' }}>
              {auction.currentPrice.toFixed(2)} zł
            </p>
          </div>
          <div>
            <p style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>Wygrywa</p>
            <p style={{ fontSize: '18px', color: '#374151' }}>
              {auction.bids.length > 0 ? auction.bids[0].bidderUsername : '—'}
            </p>
          </div>
          <div>
            <p style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>Liczba ofert</p>
            <p style={{ fontSize: '18px', color: '#374151' }}>{auction.bids.length}</p>
          </div>
          <div>
            <p style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>Start</p>
            <p style={{ color: '#374151' }}>
              {new Date(auction.startDate).toLocaleDateString('pl-PL')}
            </p>
          </div>
          <div>
            <p style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>Koniec</p>
            <p style={{ color: '#374151' }}>
              {new Date(auction.endDate).toLocaleDateString('pl-PL')}
            </p>
          </div>
        </div>

        <p style={{ fontSize: '14px', color: '#6b7280', marginBottom: '24px' }}>
          Sprzedający: <span style={{ color: '#374151' }}>{auction.ownerUsername}</span>
        </p>

        {canBid && (
          <form onSubmit={handleBid} style={{ borderTop: '1px solid #e5e7eb', paddingTop: '24px' }}>
            <h2 style={{ fontSize: '18px', fontWeight: 500, color: '#111827', marginBottom: '16px' }}>
              Złóż ofertę
            </h2>

            {bidError && (
              <div style={{ background: '#fef2f2', color: '#dc2626', padding: '12px 16px', borderRadius: '8px', fontSize: '14px', marginBottom: '16px' }}>
                {bidError}
              </div>
            )}

            <div style={{ display: 'flex', gap: '16px' }}>
              <input
                type="number"
                step="0.01"
                min={auction.currentPrice + 0.01}
                value={bidAmount}
                onChange={(e) => setBidAmount(e.target.value)}
                style={{ flex: 1, border: '1px solid #d1d5db', borderRadius: '8px', padding: '8px 16px', color: '#111827', background: '#fff' }}
              />
              <button
                type="submit"
                disabled={bidding}
                style={{
                  background: '#2563eb',
                  color: '#fff',
                  padding: '8px 24px',
                  borderRadius: '8px',
                  border: 'none',
                  cursor: 'pointer',
                  opacity: bidding ? 0.5 : 1
                }}
              >
                {bidding ? 'Wysyłanie...' : 'Licytuj'}
              </button>
            </div>
          </form>
        )}

        {auction.bids.length > 0 && (
          <div style={{ borderTop: '1px solid #e5e7eb', paddingTop: '24px', marginTop: '24px' }}>
            <h2 style={{ fontSize: '18px', fontWeight: 500, color: '#111827', marginBottom: '16px' }}>
              Historia ofert ({auction.bids.length})
            </h2>
            <div>
              {auction.bids.slice(0, 10).map((bid) => (
                <div
                  key={bid.id}
                  style={{ display: 'flex', justifyContent: 'space-between', padding: '8px 0', borderBottom: '1px solid #f3f4f6' }}
                >
                  <span style={{ color: '#4b5563' }}>{bid.bidderUsername}</span>
                  <span style={{ fontWeight: 500, color: '#111827' }}>{bid.amount.toFixed(2)} zł</span>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
