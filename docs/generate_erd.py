import matplotlib.pyplot as plt
from matplotlib.patches import FancyBboxPatch, FancyArrowPatch
from matplotlib.lines import Line2D

ENTITIES = {
    "User": {
        "pos": (0.5, 5.0),
        "color": "#dbeafe",
        "border": "#2563eb",
        "fields": [
            ("Id", "Guid", "PK"),
            ("Email", "varchar(256)", "UQ"),
            ("Username", "varchar(50)", ""),
            ("PasswordHash", "text", ""),
            ("CreatedAt", "timestamptz", ""),
        ],
    },
    "Auction": {
        "pos": (5.0, 5.0),
        "color": "#dcfce7",
        "border": "#16a34a",
        "fields": [
            ("Id", "Guid", "PK"),
            ("Title", "varchar(200)", ""),
            ("Description", "text", ""),
            ("Category", "int (enum)", ""),
            ("StartingPrice", "decimal(18,2)", ""),
            ("CurrentPrice", "decimal(18,2)", ""),
            ("StartDate", "timestamptz", ""),
            ("EndDate", "timestamptz", ""),
            ("Status", "int (enum)", ""),
            ("OwnerId", "Guid", "FK→User"),
        ],
    },
    "Bid": {
        "pos": (9.5, 5.0),
        "color": "#fef3c7",
        "border": "#d97706",
        "fields": [
            ("Id", "Guid", "PK"),
            ("Amount", "decimal(18,2)", ""),
            ("CreatedAt", "timestamptz", ""),
            ("AuctionId", "Guid", "FK→Auction"),
            ("BidderId", "Guid", "FK→User"),
        ],
    },
}

RELATIONS = [
    ("User", "Auction", "1", "N", "owns", "direct"),
    ("Auction", "Bid", "1", "N", "has", "direct"),
    ("User", "Bid", "1", "N", "places", "below"),
]


def draw_entity(ax, name, spec):
    x, y = spec["pos"]
    width = 3.6
    header_h = 0.5
    row_h = 0.32
    field_count = len(spec["fields"])
    total_h = header_h + row_h * field_count + 0.2

    body = FancyBboxPatch(
        (x, y - total_h),
        width,
        total_h,
        boxstyle="round,pad=0.02",
        linewidth=1.6,
        edgecolor=spec["border"],
        facecolor="white",
        zorder=2,
    )
    ax.add_patch(body)

    header = FancyBboxPatch(
        (x, y - header_h),
        width,
        header_h,
        boxstyle="round,pad=0.02",
        linewidth=0,
        facecolor=spec["color"],
        zorder=3,
    )
    ax.add_patch(header)

    ax.text(
        x + width / 2,
        y - header_h / 2,
        name,
        ha="center",
        va="center",
        fontsize=13,
        fontweight="bold",
        color=spec["border"],
        zorder=4,
    )

    for i, (fname, ftype, tag) in enumerate(spec["fields"]):
        cy = y - header_h - (i + 0.5) * row_h
        prefix = ""
        weight = "normal"
        if tag == "PK":
            prefix = "[PK] "
            weight = "bold"
        elif "FK" in tag:
            prefix = "[FK] "
        elif tag == "UQ":
            prefix = "[UQ] "
        ax.text(
            x + 0.15,
            cy,
            f"{prefix}{fname}",
            ha="left",
            va="center",
            fontsize=9,
            fontweight=weight,
            color="#111827",
            zorder=4,
        )
        ax.text(
            x + width - 0.15,
            cy,
            ftype if not tag or tag == "PK" else f"{ftype}  {tag.replace('FK→', '→')}",
            ha="right",
            va="center",
            fontsize=8,
            color="#6b7280",
            zorder=4,
        )

    return (x, y, width, total_h)


def draw_relation(ax, src_box, dst_box, src_card, dst_card, label, route="direct", offset=0.0):
    sx, sy, sw, sh = src_box
    dx, dy, dw, dh = dst_box

    if route == "below":
        x1 = sx + sw / 2
        x2 = dx + dw / 2
        y_src = sy - sh - 0.05
        y_bottom = -0.3 + offset
        ax.plot([x1, x1], [y_src, y_bottom], color="#374151", linewidth=1.3, zorder=1)
        ax.plot([x1, x2], [y_bottom, y_bottom], color="#374151", linewidth=1.3, zorder=1)
        arrow = FancyArrowPatch(
            (x2, y_bottom),
            (x2, sy - sh - 0.05),
            arrowstyle="-|>",
            mutation_scale=14,
            color="#374151",
            linewidth=1.3,
            zorder=1,
        )
        ax.add_patch(arrow)
        ax.text(x1 + 0.15, y_src - 0.18, src_card, fontsize=9, fontweight="bold", color="#1f2937", zorder=4)
        ax.text(x2 - 0.35, y_src - 0.18, dst_card, fontsize=9, fontweight="bold", color="#1f2937", zorder=4)
        ax.text(
            (x1 + x2) / 2,
            y_bottom - 0.25,
            label,
            fontsize=9,
            style="italic",
            color="#4b5563",
            ha="center",
            zorder=4,
        )
        return

    if sx + sw <= dx:
        x1 = sx + sw
        x2 = dx
    else:
        x1 = sx
        x2 = dx + dw

    y1 = sy - sh / 2
    y2 = dy - dh / 2

    arrow = FancyArrowPatch(
        (x1, y1),
        (x2, y2),
        arrowstyle="-|>",
        mutation_scale=14,
        color="#374151",
        linewidth=1.3,
        zorder=1,
    )
    ax.add_patch(arrow)

    mid_x = (x1 + x2) / 2
    mid_y = (y1 + y2) / 2

    ax.text(
        x1 + (0.15 if x2 > x1 else -0.15),
        y1 + 0.18,
        src_card,
        fontsize=9,
        fontweight="bold",
        color="#1f2937",
        ha="center",
        zorder=4,
    )
    ax.text(
        x2 - (0.15 if x2 > x1 else -0.15),
        y2 + 0.18,
        dst_card,
        fontsize=9,
        fontweight="bold",
        color="#1f2937",
        ha="center",
        zorder=4,
    )
    ax.text(
        mid_x,
        mid_y + 0.22,
        label,
        fontsize=9,
        style="italic",
        color="#4b5563",
        ha="center",
        zorder=4,
    )


def main():
    fig, ax = plt.subplots(figsize=(14, 9), dpi=140)
    ax.set_xlim(-0.5, 14)
    ax.set_ylim(-2.2, 6.2)
    ax.set_aspect("auto")
    ax.axis("off")

    boxes = {name: draw_entity(ax, name, spec) for name, spec in ENTITIES.items()}

    for src, dst, src_card, dst_card, label, route in RELATIONS:
        draw_relation(ax, boxes[src], boxes[dst], src_card, dst_card, label, route=route)

    ax.text(
        7,
        6.0,
        "AuctionSystem — diagram ERD",
        fontsize=16,
        fontweight="bold",
        ha="center",
        color="#111827",
    )
    ax.text(
        7,
        5.65,
        "PostgreSQL + Entity Framework Core",
        fontsize=10,
        ha="center",
        color="#6b7280",
        style="italic",
    )

    legend = [
        Line2D([0], [0], marker="s", color="w", label="[PK] Primary Key", markerfacecolor="white", markersize=10),
        Line2D([0], [0], marker="s", color="w", label="[FK] Foreign Key", markerfacecolor="white", markersize=10),
        Line2D([0], [0], marker="s", color="w", label="[UQ] Unique constraint", markerfacecolor="white", markersize=10),
    ]
    ax.legend(handles=legend, loc="lower center", ncol=3, frameon=False, fontsize=9)

    plt.tight_layout()
    plt.savefig("docs/erd.png", bbox_inches="tight", dpi=140, facecolor="white")


if __name__ == "__main__":
    main()
