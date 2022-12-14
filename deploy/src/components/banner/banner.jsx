import React from 'react';
// import { HashLink as Link } from 'react-router-hash-link';
import { Container, Row, Col, Button } from 'reactstrap';
import logo from 'assets/images/iconimg/logo03.png';

const HeaderBanner = () => {
    return (
        <div className="static-slider-head">
            <Container>
                <Row className="justify-content-center">
                    <Col lg="8" md="6" className="align-self-center text-center">
                        <img src={logo} alt="로고"></img><br/>
                        <a id="downloadButton" href='https://drive.google.com/uc?export=download&id=1QuTpq51AGaH3QMUH8K7SEvd-dQseZivy' target="_blank" rel="noopener noreferrer" download>
                        {/* <a id="downloadButton" href='./Argandion.zip' download> */}
                            <Button to="/#coming" className="btn-lg m-t-30 btn-success-gradiant font-16">
                            무료 다운로드
                            </Button>
                        </a>
                    </Col>
                </Row>
            </Container>
        </div>
    );
}

export default HeaderBanner;
