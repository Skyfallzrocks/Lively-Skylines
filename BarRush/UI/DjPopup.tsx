import React from "react";
import { bindValue, trigger, useValue } from "cs2/api";

const GROUP = "BarRush";
const visible$ = bindValue<boolean>(GROUP, "visible", false);
const djName$ = bindValue<string>(GROUP, "djName", "");
const barName$ = bindValue<string>(GROUP, "barName", "");
const attendees$ = bindValue<number>(GROUP, "attendees", 0);

const panel: React.CSSProperties = {
    position: "absolute",
    top: "92rem",
    right: "28rem",
    width: "350rem",
    boxSizing: "border-box",
    padding: "18rem 20rem 17rem",
    borderRadius: "9rem",
    border: "1rem solid rgba(255,255,255,.12)",
    borderTop: "3rem solid #e53b91",
    background: "linear-gradient(135deg, rgba(27,25,43,.97), rgba(14,20,34,.97))",
    boxShadow: "0 10rem 32rem rgba(0,0,0,.38)",
    color: "#f7f5fb",
    fontFamily: "inherit",
    pointerEvents: "auto",
};

export const DjPopup = () => {
    const visible = useValue(visible$);
    const djName = useValue(djName$);
    const barName = useValue(barName$);
    const attendees = useValue(attendees$);

    if (!visible) return null;

    return (
        <section style={panel} role="status" aria-live="polite">
            <button
                type="button"
                aria-label="Dismiss DJ announcement"
                onClick={() => trigger(GROUP, "dismiss")}
                style={{ position: "absolute", right: "12rem", top: "10rem", border: 0, background: "transparent", color: "#c4c2ce", fontSize: "20rem", cursor: "pointer" }}
            >×</button>
            <div style={{ display: "flex", alignItems: "center", gap: "7rem", color: "#ff73b6", fontSize: "10rem", fontWeight: 700, letterSpacing: "1.4rem", textTransform: "uppercase" }}>
                <span style={{ width: "7rem", height: "7rem", borderRadius: "50%", background: "#ff4f9f", boxShadow: "0 0 9rem #ff4f9f" }} />
                Live tonight <span style={{ color: "#858394" }}>·</span> DJ set
            </div>
            <div style={{ marginTop: "11rem", fontSize: "21rem", lineHeight: 1.15, fontWeight: 700 }}>{djName}</div>
            <div style={{ marginTop: "5rem", color: "#c6c4d0", fontSize: "13rem" }}>
                Playing at <strong style={{ color: "#fff" }}>{barName}</strong>
            </div>
            <div style={{ display: "flex", alignItems: "baseline", gap: "7rem", marginTop: "15rem", paddingTop: "12rem", borderTop: "1rem solid rgba(255,255,255,.1)" }}>
                <strong style={{ color: "#ff73b6", fontSize: "24rem", lineHeight: 1 }}>{attendees.toLocaleString()}</strong>
                <span style={{ color: "#d0ced9", fontSize: "12rem" }}>sims coming to the event</span>
            </div>
            <div style={{ marginTop: "8rem", color: "#858394", fontSize: "10rem" }}>Click × to dismiss · closes automatically</div>
        </section>
    );
};
