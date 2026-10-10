import React from "react";
import { bindValue, trigger, useValue } from "cs2/api";

const GROUP = "BarRush";
const barsVisible$ = bindValue<boolean>(GROUP, "barPanelVisible", false);
const eventsVisible$ = bindValue<boolean>(GROUP, "cityEventsVisible", false);
const bars$ = bindValue<string>(GROUP, "barPanelBars", "");
const events$ = bindValue<string>(GROUP, "barPanelEvents", "");
const barCount$ = bindValue<number>(GROUP, "barPanelBarCount", 0);
const eventCount$ = bindValue<number>(GROUP, "cityEventsCount", 0);

const buttonBase: React.CSSProperties = {
    minWidth: "72rem",
    height: "38rem",
    padding: "0 13rem",
    border: "1rem solid rgba(255,255,255,.18)",
    borderRadius: "6rem",
    color: "#fff",
    fontSize: "13rem",
    fontWeight: 600,
    letterSpacing: ".2rem",
    cursor: "pointer",
    pointerEvents: "auto",
    boxShadow: "0 3rem 10rem rgba(0,0,0,.28)",
};

const panelStyle: React.CSSProperties = {
    position: "fixed",
    zIndex: 1000,
    top: "60rem",
    left: "50%",
    transform: "translateX(-50%)",
    width: "430rem",
    maxHeight: "70vh",
    display: "flex",
    flexDirection: "column",
    boxSizing: "border-box",
    padding: "18rem 20rem",
    border: "1rem solid rgba(255,255,255,.14)",
    borderRadius: "10rem",
    background: "rgba(20,24,40,.97)",
    boxShadow: "0 12rem 36rem rgba(0,0,0,.42)",
    color: "#f7f5fb",
    fontSize: "13rem",
    pointerEvents: "auto",
};

const rows = (value: string) => value ? value.split("\n").filter(Boolean) : [];

export const BarDashboard = () => {
    const barsVisible = useValue(barsVisible$);
    const eventsVisible = useValue(eventsVisible$);
    const bars = rows(useValue(bars$));
    const events = rows(useValue(events$));
    const barCount = useValue(barCount$);
    const eventCount = useValue(eventCount$);
    const isEvents = eventsVisible;
    const open = barsVisible || eventsVisible;
    const list = isEvents ? events : bars;
    const title = isEvents ? "City Events" : "Bars";
    const count = isEvents ? eventCount : barCount;

    return (
        <>
            <div style={{ position: "fixed", zIndex: 1001, top: "10rem", left: "50%", transform: "translateX(-50%)", display: "flex", gap: "7rem", pointerEvents: "none" }}>
                <button type="button" title="Show bars in your city" onClick={() => trigger(GROUP, "toggleBarPanel")} style={{ ...buttonBase, background: barsVisible ? "#1f5fa6" : "#2f86d6" }}>Bars</button>
                <button type="button" title="Show current city events" onClick={() => trigger(GROUP, "toggleCityEvents")} style={{ ...buttonBase, background: eventsVisible ? "#1f5fa6" : "#2f86d6" }}>City Events</button>
            </div>
            {open && (
                <section style={panelStyle} role="dialog" aria-label={title}>
                    <header style={{ display: "flex", alignItems: "center", justifyContent: "space-between", paddingBottom: "12rem", borderBottom: "1rem solid rgba(255,255,255,.12)" }}>
                        <div>
                            <div style={{ color: "#9bbce0", fontSize: "10rem", fontWeight: 700, letterSpacing: "1.2rem", textTransform: "uppercase" }}>Bar Rush</div>
                            <div style={{ marginTop: "3rem", fontSize: "20rem", fontWeight: 700 }}>{title} <span style={{ color: "#aab4c3", fontSize: "14rem", fontWeight: 500 }}>({count})</span></div>
                        </div>
                        <button type="button" aria-label="Close panel" onClick={() => trigger(GROUP, isEvents ? "closeCityEvents" : "closeBarPanel")} style={{ border: 0, background: "transparent", color: "#d8dbe2", fontSize: "24rem", cursor: "pointer" }}>×</button>
                    </header>
                    <div style={{ overflowY: "auto", minHeight: "80rem", paddingTop: "10rem" }}>
                        {list.length === 0 ? (
                            <div style={{ padding: "15rem 2rem", color: "#bdc4d0" }}>{isEvents ? "No active city events right now." : "No entertainment businesses found yet."}</div>
                        ) : list.map((line, i) => (
                            <div key={`${i}-${line}`} style={{ padding: "11rem 3rem", borderBottom: "1rem solid rgba(255,255,255,.08)", lineHeight: 1.35 }}>{line}</div>
                        ))}
                    </div>
                </section>
            )}
        </>
    );
};
